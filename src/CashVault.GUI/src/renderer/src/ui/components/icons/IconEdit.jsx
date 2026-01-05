/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import { sizeMap } from './size-map'
import { IconContainer } from './styles'

const IconEdit = ({ color = 'black', size = 's' }) => {
  return (
    <IconContainer color={color} w={sizeMap[size].w} h={sizeMap[size].h}>
      <svg
        width="28"
        height="28"
        viewBox="0 0 28 28"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >
        <path
          d="M4.66699 24.4997C4.66699 23.8553 5.18933 23.333 5.83366 23.333H22.167C22.8113 23.333 23.3337 23.8553 23.3337 24.4997C23.3337 25.144 22.8113 25.6663 22.167 25.6663H5.83366C5.18933 25.6663 4.66699 25.144 4.66699 24.4997Z"
          fill="black"
        />
        <path
          fillRule="evenodd"
          clipRule="evenodd"
          d="M19.4304 5.35269C18.5191 4.44147 17.0418 4.44147 16.1305 5.35268L8.07343 13.4097L7.00265 18.6641L12.2568 17.5932L20.3139 9.5361C21.2252 8.62488 21.2252 7.14749 20.3139 6.23627L19.4304 5.35269ZM14.4806 3.70276C16.3031 1.88032 19.2578 1.88033 21.0803 3.70277L21.9639 4.58635C23.7863 6.4088 23.7863 9.36358 21.9639 11.186L13.9067 19.2431C13.5831 19.5668 13.1714 19.7881 12.7228 19.8795L7.46866 20.9504C5.83001 21.2844 4.38237 19.8368 4.71631 18.1981L5.78709 12.9438C5.8785 12.4953 6.09983 12.0835 6.42352 11.7598L14.4806 3.70276Z"
          fill="black"
        />
      </svg>
    </IconContainer>
  )
}

export default IconEdit