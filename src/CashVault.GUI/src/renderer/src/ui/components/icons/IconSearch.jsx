/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import { sizeMap } from './size-map'
import { IconContainer } from './styles'

const IconSearch = ({ color = 'black', size = 's' }) => {
  return (
    <IconContainer color={color} w={sizeMap[size].w} h={sizeMap[size].h}>
      <svg
        width="48"
        height="48"
        viewBox="0 0 48 48"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >
        <path
          fillRule="evenodd"
          clipRule="evenodd"
          d="M27.5858 27.5858C28.3668 26.8047 29.6332 26.8047 30.4142 27.5858L41.4142 38.5858C42.1953 39.3668 42.1953 40.6332 41.4142 41.4142C40.6332 42.1953 39.3668 42.1953 38.5858 41.4142L27.5858 30.4142C26.8047 29.6332 26.8047 28.3668 27.5858 27.5858Z"
          fill="currentColor"
        />
        <path
          fillRule="evenodd"
          clipRule="evenodd"
          d="M20 10C14.4772 10 10 14.4772 10 20C10 25.5228 14.4772 30 20 30C25.5228 30 30 25.5228 30 20C30 14.4772 25.5228 10 20 10ZM6 20C6 12.268 12.268 6 20 6C27.732 6 34 12.268 34 20C34 27.732 27.732 34 20 34C12.268 34 6 27.732 6 20Z"
          fill="currentColor"
        />
      </svg>
    </IconContainer>
  )
}

export default IconSearch
