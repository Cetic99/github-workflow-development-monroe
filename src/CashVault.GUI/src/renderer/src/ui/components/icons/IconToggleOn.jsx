/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import { sizeMap } from './size-map'
import { IconContainer } from './styles'

const IconToggleOn = ({ color = 'black', size = 's' }) => {
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
          d="M2 12C2 8.68629 4.68629 6 8 6H16C19.3137 6 22 8.68629 22 12C22 15.3137 19.3137 18 16 18H8C4.68629 18 2 15.3137 2 12ZM8 8C5.79086 8 4 9.79086 4 12C4 14.2091 5.79086 16 8 16H16C18.2091 16 20 14.2091 20 12C20 9.79086 18.2091 8 16 8H8Z"
          fill="currentColor"
        />
        <path
          d="M18 12C18 13.1046 17.1046 14 16 14C14.8954 14 14 13.1046 14 12C14 10.8954 14.8954 10 16 10C17.1046 10 18 10.8954 18 12Z"
          fill="currentColor"
        />
      </svg>
    </IconContainer>
  )
}

export default IconToggleOn
