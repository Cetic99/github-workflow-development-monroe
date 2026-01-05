/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import { sizeMap } from './size-map'
import { IconContainer } from './styles'

const IconRedo = ({ color = 'black', size = 's' }) => {
  return (
    <IconContainer color={color} w={sizeMap[size].w} h={sizeMap[size].h}>
      <svg
        width="24"
        height="24"
        viewBox="0 0 24 24"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >
        <path
          fillRule="evenodd"
          clipRule="evenodd"
          d="M22 9C22 8.44772 21.5523 8 21 8H8C4.68629 8 2 10.6863 2 14C2 17.3137 4.68629 20 8 20H19C19.5523 20 20 19.5523 20 19C20 18.4477 19.5523 18 19 18H8C5.79086 18 4 16.2091 4 14C4 11.7909 5.79086 10 8 10H21C21.5523 10 22 9.55228 22 9Z"
          fill="currentColor"
        />
        <path
          fillRule="evenodd"
          clipRule="evenodd"
          d="M21.7071 9.70711C22.0976 9.31658 22.0976 8.68342 21.7071 8.29289L17.7071 4.29289C17.3166 3.90237 16.6834 3.90237 16.2929 4.29289C15.9024 4.68342 15.9024 5.31658 16.2929 5.70711L19.5858 9L16.2929 12.2929C15.9024 12.6834 15.9024 13.3166 16.2929 13.7071C16.6834 14.0976 17.3166 14.0976 17.7071 13.7071L21.7071 9.70711Z"
          fill="currentColor"
        />
      </svg>
    </IconContainer>
  )
}

export default IconRedo
